using MetaBond.Application.DTOs.Account.CommunityMembership;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class CommunityMembershipMapper
{
    public static CommunityMembershipDto ModelToDto(CommunityMembership model)
    {
        return new CommunityMembershipDto
        (
            CommunityMembershipId: model.Id,
            User: UserMapper.MapUserCommunityMembershipDto(model.User!),
            Community: CommunityMapper.MapCommunitiesDTos(model.Community!),
            Role: model.Role,
            IsActive: model.IsActive
        );
    }

    public static CommunityMembersDto CommunityMembersToDto(CommunityMembership communityMembers)
    {
        return new CommunityMembersDto
        (
            CommunityMembershipId: communityMembers.Id,
            User: UserMapper.MapUserDTos(communityMembers.User!),
            Role: communityMembers.Role
        );
    }

    public static CommunitiesDTos CommunityMembershipToCommunitiesDTos(CommunityMembership communityMembership)
    {
        return CommunityMapper.MapCommunitiesDTos(communityMembership.Community!);
    }

    public static LeaveCommunityDto LeaveCommunityToDto(CommunityMembership communityMembership)
    {
        return new LeaveCommunityDto
        (
            communityMembership.IsActive,
            communityMembership.LeftOnUtc
        );
    }
}