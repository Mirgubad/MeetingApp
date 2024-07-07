using MediatR;
using MeetApp.Infrastructure.Entities.Membership;
using MeetApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MeetApp.Business.Modules.MeetingModule.Commands.MeetingJoinCommand
{
    internal class MeetingJoinRequestHandler : IRequestHandler<MeetingJoinRequest, MeetingJoinDto>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IHttpContextAccessor _httpCtx;
        private readonly UserManager<MeetAppUser> _userManager;

        public MeetingJoinRequestHandler(
            IMeetingRepository meetingRepository,
            IHttpContextAccessor httpCtx,
            UserManager<MeetAppUser> userManager)
        {
            _meetingRepository = meetingRepository;
            _httpCtx = httpCtx;
            _userManager = userManager;
        }
        public async Task<MeetingJoinDto> Handle(MeetingJoinRequest request, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository
                .GetAsync(x => x.Id.Equals(request.MeetingId) && !x.IsClosed);

            if (meeting == null)
            {
                throw new Exception("Link is broken");
            }

            var userMeeting = await _userManager.FindByNameAsync(_httpCtx.HttpContext.User.Identity.Name);

            if (userMeeting == null)
            {
                throw new Exception("Link is broken");
            }

            var dto = new MeetingJoinDto
            {
                MeetingId = meeting.Id,
                UserFullname = userMeeting.Fullname
            };

            return dto;

        }
    }
}
