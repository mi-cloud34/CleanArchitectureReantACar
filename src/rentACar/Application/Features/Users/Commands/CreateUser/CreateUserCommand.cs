
using Application.Features.Users.Constants;
using Application.Features.Users.Dtos;
using Application.Features.Users.Dtos.EventUserBus;
using Application.Features.Users.Rules;
using Application.Services.AzureBus;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Mailing;
using Core.Security.Entities;
using Core.Security.Hashing;
using Core.Storage;
using MediatR;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System.Collections.ObjectModel;
using System.Net.Mail;
using static Application.Features.Users.Constants.OperationClaims;
using static Domain.Constants.OperationClaims;

namespace Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<CreatedUserDto>, ISecuredRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public IFormFile? Files { get; set; }

  
    public string[] Roles => new[] { Admin, UserAdd };

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreatedUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IAzureBusService<User> _azureBusService;
        private readonly IStorageService _storageServices;
        private readonly IMailService _mailService;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper,
                                        UserBusinessRules userBusinessRules,IAzureBusService<User> azureBusService,IStorageService storageService,IMailService mailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _azureBusService = azureBusService;
            _storageServices=storageService;
            _mailService = mailService;
        }

        public async Task<CreatedUserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            User mappedUser = _mapper.Map<User>(request);

            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);
            mappedUser.PasswordHash = passwordHash;
            mappedUser.PasswordSalt = passwordSalt;
           ;
            (string fileName, string pathOrContainerName) result = await _storageServices.UploadAsync("user-files", request.Files);
            mappedUser.UserFileImages = new Collection<UserFileImage>()
            {
                new UserFileImage(){
                Name = result.fileName,
                Path = result.pathOrContainerName,
                Storage = _storageServices.StorageName,
                UserId=mappedUser.Id
                }
            };

            User createdUser = await _userRepository.AddAsync(mappedUser);
            CreatedUserDto createdUserDto = _mapper.Map<CreatedUserDto>(createdUser);
            var userCreatedEvent = new CreatedEventUser()
            {
                Id = createdUser.Id,
                CreatedAt = DateTime.Now,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email

            };
            if (createdUserDto != null)
            {
                await _azureBusService.CreateQueeIfNotExists(UserMessage.UserCreatedQuee);
                await _azureBusService.sendMessageToQuee(userCreatedEvent, UserMessage.UserCreatedQuee);

            }
            Mail mail = new Mail()
            {
          Subject=UserMessage.UserMailSubject,
          TextBody=UserMessage.UserMailBody,
          //HtmlBody="",
          //Attachments=
          ToFullName=mappedUser.FirstName,
          ToEmail=mappedUser.Email,
          
    };
            await _azureBusService.ConsumerQuee<CreatedEventUser>(UserMessage.UserCreatedQuee, i =>
            {
                Console.WriteLine($"UserCreate Event Receivid with id :${i.Id} userName:${i.FirstName} userLastName:${i.LastName}");
                userCreatedEvent.Id = i.Id;
                userCreatedEvent.FirstName = i.FirstName;
                userCreatedEvent.LastName = i.LastName;
                userCreatedEvent.Email = i.Email;
                userCreatedEvent.CreatedDate = DateTime.Now;
                _mailService.SendMail(mail);
            });
            return createdUserDto;
        }
    }
}