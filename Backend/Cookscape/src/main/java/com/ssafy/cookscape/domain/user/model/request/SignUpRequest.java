package com.ssafy.cookscape.domain.user.model.request;

import com.ssafy.cookscape.domain.information.db.entity.AvatarEntity;
import com.ssafy.cookscape.domain.data.db.entity.DataEntity;
import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class SignUpRequest {

    private String loginId;
    private String password;
    private String nickname;

    public UserEntity toEntity(DataEntity data){
        return UserEntity.builder()
                .data(data)
                .avatarName("고기맨")
                .loginId(this.loginId)
                .password(this.password)
                .nickname(this.nickname)
                .title("무기농")
                .hat("NONE")
                .build();
    }
}
