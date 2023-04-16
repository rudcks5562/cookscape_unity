package com.ssafy.cookscape.domain.user.model.response;

import com.ssafy.cookscape.domain.information.db.entity.AvatarEntity;
import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class UserResponse {

    private Long userId;
    private String nickname;
    private String avatarName;
    private String title;
    private String hat;

    public static UserResponse toDto(UserEntity user){
        return UserResponse.builder()
                .userId(user.getId())
                .nickname(user.getNickname())
                .avatarName(user.getAvatarName())
                .title(user.getTitle())
                .hat(user.getHat())
                .build();
    }
}
