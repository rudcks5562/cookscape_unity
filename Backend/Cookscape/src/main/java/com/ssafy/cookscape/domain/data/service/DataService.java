package com.ssafy.cookscape.domain.data.service;

import com.ssafy.cookscape.domain.data.db.entity.UserRewardEntity;
import com.ssafy.cookscape.domain.data.db.repository.UserRewardRepository;
import com.ssafy.cookscape.domain.data.model.request.GameResultRequest;
import com.ssafy.cookscape.domain.information.db.entity.RewardEntity;
import com.ssafy.cookscape.domain.information.db.repository.RewardRepository;
import com.ssafy.cookscape.domain.information.model.response.RewardResponse;
import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import com.ssafy.cookscape.global.error.exception.CustomException;
import com.ssafy.cookscape.global.util.JwtUtil;
import org.springframework.http.HttpHeaders;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

import com.ssafy.cookscape.domain.data.db.entity.DataEntity;
import com.ssafy.cookscape.domain.data.db.entity.UserAvatarEntity;
import com.ssafy.cookscape.domain.data.model.response.UsageAvatarResponse;
import com.ssafy.cookscape.domain.information.db.entity.AvatarEntity;
import com.ssafy.cookscape.domain.information.db.repository.AvatarRepository;
import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import com.ssafy.cookscape.domain.data.db.repository.DataRepository;
import com.ssafy.cookscape.domain.data.db.repository.UserAvatarRepository;
import com.ssafy.cookscape.domain.user.db.repository.UserRepository;
import com.ssafy.cookscape.global.util.ResponseUtil;
import com.ssafy.cookscape.global.error.exception.ErrorCode;
import com.ssafy.cookscape.domain.data.model.response.DataResponse;

import lombok.RequiredArgsConstructor;

import javax.servlet.http.HttpServletRequest;

@Service
@RequiredArgsConstructor // Lombok을 사용해 @Autowired 없이 의존성 주입. final 객제만 주입됨을 주의
public class DataService {

    private final ResponseUtil responseUtil;
    private final JwtUtil jwtUtil;
    private final DataRepository dataRepository;
    private final UserRepository userRepository;
    private final AvatarRepository avatarRepository;
    private final UserAvatarRepository userAvatarRepository;
    private final UserRewardRepository userRewardRepository;
    private final RewardRepository rewardRepository;

    // 회원가입 시 유저에 대한 모든 기본데이터에 대한 정보 생성
    @Transactional
    public Long addUserData(){

        DataEntity saveUserData = DataEntity.builder().level(1).rankPoint(1000).build();

        long id = dataRepository.save(saveUserData).getId();

        return id;
    }

    // 회원가입 시 유저에 대한 데이터 생성
    @Transactional
    public SuccessResponse<String> initUserData(Long userId){

        UserEntity findUser = userRepository.findById(userId)
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        List<AvatarEntity> findAvatarList = avatarRepository.findAll();

        List<UserAvatarEntity> saveUserAvatarList = new ArrayList<>();

        for(AvatarEntity avatar : findAvatarList){
            saveUserAvatarList.add(UserAvatarEntity.builder()
                    .avatar(avatar)
                    .user(findUser)
                    .build());
        }

        userAvatarRepository.saveAll(saveUserAvatarList);

        RewardEntity findReward = rewardRepository.findByKeyValueLike("무기농");
        if(null == findReward){
            throw new CustomException(ErrorCode.REWARD_NOT_FOUND);
        }

        UserRewardEntity userReward = UserRewardEntity.builder()
                .user(findUser)
                .reward(findReward)
                .build();

        userRewardRepository.save(userReward);

        return responseUtil.buildSuccessResponse("가입되었습니다.");

    }

    // 유저가 사용한 아바타 조회
    @Transactional(readOnly = true)
    public SuccessResponse<List<UsageAvatarResponse>> getUsageAvatarData(HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        List<UserAvatarEntity> findUserAvatarList = userAvatarRepository.findByUser(loginUser);

        List<UsageAvatarResponse> UsageAvatarList = findUserAvatarList.stream()
                .map(UsageAvatarResponse::toDto)
                .collect(Collectors.toList());

        return responseUtil.buildSuccessResponse(UsageAvatarList);
    }

    // 유저 데이터 조회
    @Transactional(readOnly = true)
    public SuccessResponse<DataResponse> getUserData(HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        DataResponse UserData = DataResponse.toDto(loginUser.getData());

        return responseUtil.buildSuccessResponse(UserData);

    }

    // 게임 결과 데이터를 업데이트
    @Transactional
    public SuccessResponse<String> updateResultData(GameResultRequest resultDto, HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        DataEntity findData = dataRepository.findByUser(loginUser)
                .orElseThrow(() -> new CustomException(ErrorCode.USER_DATA_NOT_FOUND));

        AvatarEntity findAvatar = avatarRepository.findById(resultDto.getAvatarId())
                .orElseThrow(() -> new CustomException(ErrorCode.AVATAR_NOT_FOUND));

        UserAvatarEntity findUserAvatar = userAvatarRepository.findByUserAndAvatar(loginUser, findAvatar)
                .orElseThrow(() -> new CustomException(ErrorCode.USER_AVATAR_NOT_FOUND));


        UserAvatarEntity saveUserAvatar = resultDto.toUserAvatarEntity(findUserAvatar);

        DataEntity saveData = resultDto.toDataEntity(findData);

        dataRepository.save(saveData).getId();
        userAvatarRepository.save(saveUserAvatar);

        return responseUtil.buildSuccessResponse("수정되었습니다.");
    }

    // 보상 수령 등록
    @Transactional
    public SuccessResponse<String> receiveReward(Long rewardId, HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        RewardEntity findReward = rewardRepository.findByIdAndExpiredLike(rewardId, "F")
                .orElseThrow(() -> new CustomException(ErrorCode.REWARD_NOT_FOUND));

        if(null != userRewardRepository.findByUserAndReward(loginUser, findReward)){
            throw new CustomException(ErrorCode.DUPLICATE_REWARD);
        }

        UserRewardEntity saveUserReward = UserRewardEntity.builder()
                .user(loginUser)
                .reward(findReward)
                .build();

        userRewardRepository.save(saveUserReward);

        return responseUtil.buildSuccessResponse("등록되었습니다.");
    }

    //보유/미보유 보상을 타입별로 조회
    @Transactional(readOnly = true)
    public SuccessResponse<List<RewardResponse>> getRewardByConditon(Boolean isPossession, HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        List<RewardResponse> userRewardResponseList = new ArrayList<>();

        if(isPossession){
            List<UserRewardEntity> userRewardList = userRewardRepository.findByUser(loginUser);
            for(UserRewardEntity userReward : userRewardList){
                userRewardResponseList.add(RewardResponse.toDto(userReward.getReward()));
            }
        }
        else{
            List<RewardEntity> allRewardList = rewardRepository.findByExpiredLike("F");
            for(RewardEntity reward : allRewardList){
                UserRewardEntity findUserReward = userRewardRepository.findByUserAndReward(loginUser, reward);
                if(null == findUserReward){
                    userRewardResponseList.add(RewardResponse.toDto(reward));
                }
            }
        }

        return responseUtil.buildSuccessResponse(userRewardResponseList);
    }
}
