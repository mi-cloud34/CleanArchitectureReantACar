using Application.Features.Users.Constants;
using Application.Features.Users.Dtos;
using Application.Features.Users.Dtos.EventUserBus;
using Application.Features.Users.Rules;
using Application.Services.AzureBus;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Security.Entities;
using MediatR;
using static Application.Features.Users.Constants.OperationClaims;
using static Domain.Constants.OperationClaims;

namespace Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<DeletedUserDto>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { Admin, UserDelete };

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeletedUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IAzureBusService<User> _azureBusService;
        public DeleteUserCommandHandler(IUserRepository userRepository, IMapper mapper,
                                        UserBusinessRules userBusinessRules,IAzureBusService<User> azureBusService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<DeletedUserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldExistWhenSelected(request.Id);

            User mappedUser = _mapper.Map<User>(request);
            User deletedUser = await _userRepository.DeleteAsync(mappedUser);
            DeletedUserDto deletedUserDto = _mapper.Map<DeletedUserDto>(deletedUser);
            //var userDeletedEvent = new DeleteEventUser()
            //{
            //    CreatedAt = DateTime.Now,
            //    Id = deletedUserDto.Id,
                

            //};
            //if (deletedUserDto != null)
            //{
            //    await _azureBusService.CreateQueeIfNotExists(UserMessage.UserDeletedQuee);
            //    await _azureBusService.sendMessageToQuee(userDeletedEvent, UserMessage.UserDeletedQuee);

            //}
            //await _azureBusService.ConsumerQuee<DeleteEventUser>(UserMessage.UserDeletedQuee, i =>
            //{
            //    Console.WriteLine($"UserCreate Event Receivid with id :${i.Id} userName:${i.FirstName} userLastName:${i.LastName}");
            //});
            return deletedUserDto;
        }
    }
}