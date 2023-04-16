package com.ssafy.cookscape.domain.data.model.response;

import com.ssafy.cookscape.domain.data.db.entity.UserAvatarEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class UsageAvatarResponse {

    private Long userAvatarId;
    private Long avatarId;
    private String name;
    private String desc;
    private int useCount;

    public static UsageAvatarResponse toDto(UserAvatarEntity userAvatar){

        return UsageAvatarResponse.builder()
                .userAvatarId(userAvatar.getId())
                .avatarId(userAvatar.getAvatar().getId())
                .name(userAvatar.getAvatar().getName())
                .desc(userAvatar.getAvatar().getDescription())
                .useCount(userAvatar.getUseCount())
                .build();
    }
}
