using MediatR;
using MeetApp.Infrastructure.Entities;
using MeetApp.Infrastructure.Entities.Membership;
using MeetApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MeetApp.Business.Modules.MeetingModule.Commands.MeetingCreateCommand
{
    internal class MeetingCreateRequestHandler : IRequestHandler<MeetingCreateRequest, MeetingCreateDto>
    {
        private readonly UserManager<MeetAppUser> _userManager;
        private readonly IMeetingRepository _meetingRepository;
        private readonly IHttpContextAccessor _httpctx;

        public MeetingCreateRequestHandler(
            UserManager<MeetAppUser> userManager,
            IMeetingRepository meetingRepository,
            IHttpContextAccessor httpctx)
        {
            _userManager = userManager;
            _meetingRepository = meetingRepository;
            _httpctx = httpctx;
        }


        public async Task<MeetingCreateDto> Handle(MeetingCreateRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpctx.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                throw new Exception("User ID is null");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception($"User not found for ID: {userId}");
            }

            var meeting = new Meeting
            {
                OrganizerId = user.Id,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
            };

            await _meetingRepository.AddAsync(meeting);


            var dto = new MeetingCreateDto
            {
                MeetingId = meeting.Id,
                UserFullname = user.Fullname,
                IsClosed = meeting.IsClosed,
                EndTime = meeting.EndTime,
                NickName = request.NickName
            };

            return dto;
        }
    }
}
