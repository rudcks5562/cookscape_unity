package com.ssafy.cookscape.domain.user.model.request;

import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;

@Getter
@Setter
@Builder
public class UpdateUserRequest {

    private String avatarName;
    private String title;
    private String hat;

    public UserEntity toEntity(UserEntity user){
        return UserEntity.builder()
                .id(user.getId())
                .loginId(user.getLoginId())
                .password(user.getPassword())
                .nickname(user.getNickname())
                .data(user.getData())
                .avatarName(avatarName)
                .title(title)
                .hat(hat)
                .createdDate(user.getCreatedDate())
                .updatedDate(LocalDateTime.now())
                .expired(user.getExpired())
                .build();
    }
}
