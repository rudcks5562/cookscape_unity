package com.ssafy.cookscape.domain.user.service;

import com.ssafy.cookscape.domain.data.db.entity.DataEntity;
import com.ssafy.cookscape.domain.data.db.repository.UserRewardRepository;
import com.ssafy.cookscape.domain.information.db.entity.RewardEntity;
import com.ssafy.cookscape.domain.information.db.repository.RewardRepository;
import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import com.ssafy.cookscape.domain.information.db.repository.AvatarRepository;
import com.ssafy.cookscape.domain.data.db.repository.DataRepository;
import com.ssafy.cookscape.domain.user.db.repository.UserRepository;
import com.ssafy.cookscape.domain.user.model.request.UpdateUserRequest;
import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import com.ssafy.cookscape.global.error.exception.CustomException;
import com.ssafy.cookscape.global.util.CookieUtil;
import com.ssafy.cookscape.global.util.JwtUtil;
import com.ssafy.cookscape.global.error.exception.ErrorCode;
import com.ssafy.cookscape.domain.user.model.request.SignInRequest;
import com.ssafy.cookscape.domain.user.model.response.UserResponse;
import org.springframework.http.HttpHeaders;
import org.springframework.http.ResponseCookie;
import org.springframework.http.ResponseEntity;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.ssafy.cookscape.global.util.ResponseUtil;
import com.ssafy.cookscape.domain.user.model.request.SignUpRequest;

import lombok.RequiredArgsConstructor;

import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import java.util.Optional;

@Service
@Transactional(readOnly = true) // 기본적으로 트랜잭션 안에서만 데이터 변경하게 설정(성능 향상)
@RequiredArgsConstructor // Lombok을 사용해 @Autowired 없이 의존성 주입. final 객제만 주입됨을 주의
public class UserService {

    private final PasswordEncoder passwordEncoder;

    private final CookieUtil cookieUtil;
    private final JwtUtil jwtUtil;
    private final ResponseUtil responseUtil;

    private final UserRepository userRepository;
    private final DataRepository dataRepository;
    private final AvatarRepository avatarRepository;
    private final RewardRepository rewardRepository;
    private final UserRewardRepository userRewardRepository;


    /*
        회원 가입시 userData, userAvatar 정보도 포함해야 한다.
     */
    @Transactional
    public Long signUp(SignUpRequest userDto, Long userDataId){

        userDto.setPassword(passwordEncoder.encode(userDto.getPassword()));

        DataEntity findUserData = dataRepository.findById(userDataId)
                .orElseThrow(() -> new CustomException(ErrorCode.USER_DATA_NOT_FOUND));

        UserEntity user = userDto.toEntity(findUserData);

        Long userId = userRepository.save(user).getId();

        return userId;
    }

    public HttpHeaders signIn(SignInRequest signInDto){

        UserEntity findUser = userRepository.findByLoginIdAndExpiredLike(signInDto.getLoginId(), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        if(!passwordEncoder.matches(signInDto.getPassword(), findUser.getPassword())){
            throw new CustomException(ErrorCode.INVALID_PASSWORD);
        }

        String accessJwt = jwtUtil.createAccessToken(findUser);
        String refreshJwt = jwtUtil.createRefreshToken(findUser);

        Cookie refreshToken = cookieUtil.createCookie(jwtUtil.REFRESH_TOKEN, refreshJwt);
        ResponseCookie refreshCookie = cookieUtil.toResponseCookie(refreshToken);

        HttpHeaders headers = new HttpHeaders();
        headers.set(HttpHeaders.SET_COOKIE, refreshCookie.toString());
        headers.set(HttpHeaders.AUTHORIZATION, accessJwt);

        return headers;
    }

    public SuccessResponse<UserResponse> getUser(HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        UserResponse userResponse = UserResponse.toDto(loginUser);

        return responseUtil.buildSuccessResponse(userResponse);
    }

    public SuccessResponse<String> checkId(String loginId){

        if(null != userRepository.findByLoginId(loginId)){
            throw new CustomException(ErrorCode.DUPLICATE_ID);
        }

        return responseUtil.buildSuccessResponse(loginId);
    }

    public SuccessResponse<String> checkNickname(String nickname){

        if(null != userRepository.findByNickname(nickname)){
            throw new CustomException(ErrorCode.DUPLICATE_NICKNAME);
        }

        return responseUtil.buildSuccessResponse(nickname);
    }

    @Transactional
    public SuccessResponse<String> updateUser(UpdateUserRequest updateUserDto, HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));



        RewardEntity findTitle = rewardRepository.findByKeyValueLike(updateUserDto.getTitle());
        if(!"NONE".equals(updateUserDto.getTitle()) && null == userRewardRepository.findByUserAndReward(loginUser, findTitle)){
            throw new CustomException(ErrorCode.INVALID_USER_TITLE);
        }

        RewardEntity findHat = rewardRepository.findByKeyValueLike(updateUserDto.getHat());
        if(!"NONE".equals(updateUserDto.getHat()) && null == userRewardRepository.findByUserAndReward(loginUser, findHat)){
            throw new CustomException(ErrorCode.INVALID_USER_HAT);
        }

        UserEntity saveUser = updateUserDto.toEntity(loginUser);
        userRepository.save(saveUser);

        return responseUtil.buildSuccessResponse("수정 되었습니다.");
    }

}
