package com.ssafy.cookscape.domain.user.controller;

import com.ssafy.cookscape.domain.user.model.request.*;
import com.ssafy.cookscape.domain.data.service.DataService;
import com.ssafy.cookscape.domain.user.service.UserService;
import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpServletRequest;

@Slf4j
@RestController
@RequiredArgsConstructor
@Api(tags = "UserController v1")
@RequestMapping("/v1/user")
public class UserController {

    private final UserService userService;
    private final DataService dataService;

    // 회원가입
    // 회원가입 성공 시 유저에 대한 모든 기본 유저-데이터, 유저-아바타 테이블 생성
    @PostMapping("/signup")
    @ApiOperation(value = "회원 가입")
    public ResponseEntity<SuccessResponse> signUp(@RequestBody SignUpRequest signUpDto){

        Long dataId = dataService.addUserData();
        Long userId = userService.signUp(signUpDto, dataId);
        SuccessResponse body = dataService.initUserData(userId);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(body);
    }

    // 로그인
    @PostMapping("/signin")
    @ApiOperation(value = "로그인")
    public ResponseEntity<String> signIn(@RequestBody SignInRequest signInDto){

        HttpHeaders headers = userService.signIn(signInDto);
        String body = "로그인 되었습니다.";

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .headers(headers)
                .body(body);
    }

    // 아이디 중복체크
    @GetMapping("/id-check/{loginId}")
    @ApiOperation(value = "아이디 중복 체크")
    public ResponseEntity<SuccessResponse> checkId(@PathVariable String loginId){

        SuccessResponse body = userService.checkId(loginId);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }

    // 닉네임 중복체크
    @GetMapping("/nickname-check/{nickname}")
    @ApiOperation(value = "닉네임 중복 체크")
    public ResponseEntity<SuccessResponse> checkNickname(@PathVariable String nickname){

        SuccessResponse body = userService.checkNickname(nickname);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }

    @GetMapping
    @ApiOperation(value = "유저 정보 조회")
    public ResponseEntity<SuccessResponse> getUser(HttpServletRequest request){

        SuccessResponse body = userService.getUser(request);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }

    @PutMapping
    @ApiOperation(value = "유저 아바타/칭호/모자 변경")
    public ResponseEntity<SuccessResponse> changeAvatar(@RequestBody UpdateUserRequest updateUserDto, HttpServletRequest request){

        SuccessResponse body = userService.updateUser(updateUserDto, request);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(body);
    }
}
