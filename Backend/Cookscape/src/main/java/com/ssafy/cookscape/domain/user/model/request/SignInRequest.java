package com.ssafy.cookscape.domain.user.model.request;

import lombok.Builder;
import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@Builder
@ToString
public class SignInRequest {

    private String loginId;
    private String password;

}
