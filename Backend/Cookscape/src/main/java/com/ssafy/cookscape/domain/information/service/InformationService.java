package com.ssafy.cookscape.domain.information.service;

import com.ssafy.cookscape.domain.data.db.entity.UserRewardEntity;
import com.ssafy.cookscape.domain.data.db.repository.UserRewardRepository;
import com.ssafy.cookscape.domain.information.db.entity.*;
import com.ssafy.cookscape.domain.information.db.repository.*;
import com.ssafy.cookscape.domain.information.model.response.*;
import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import com.ssafy.cookscape.domain.user.db.repository.UserRepository;
import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import com.ssafy.cookscape.global.error.exception.CustomException;
import com.ssafy.cookscape.global.util.JwtUtil;
import com.ssafy.cookscape.global.error.exception.ErrorCode;
import org.springframework.http.HttpHeaders;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

import com.ssafy.cookscape.global.util.ResponseUtil;

import lombok.RequiredArgsConstructor;

import javax.servlet.http.HttpServletRequest;

@Service
@RequiredArgsConstructor // Lombok을 사용해 @Autowired 없이 의존성 주입. final 객제만 주입됨을 주의
public class InformationService {

    private final ResponseUtil responseUtil;
    private final JwtUtil jwtUtil;

    private final UserRepository userRepository;
    private final AvatarRepository avatarRepository;
    private final ItemRepository itemRepository;
    private final ObjectRepository objectRepository;
    private final ChallengeRepository challengeRepository;
    private final UserRewardRepository userRewardRepository;
    private final RewardRepository rewardRepository;

    // 아이템 정보 모두 조회
    @Transactional(readOnly = true)
    public SuccessResponse<List<ItemResponse>> getAllItem(HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        List<ItemEntity> itemEntityList = itemRepository.findByExpiredLike("F");
        List<ItemResponse> itemResponseList = itemEntityList.stream()
                .map(ItemResponse::toDto)
                .collect(Collectors.toList());

        return responseUtil.buildSuccessResponse(itemResponseList);
    }

    // 아바타 정보 모두 조회
    @Transactional(readOnly = true)
    public SuccessResponse<List<AvatarResponse>> getAllAvatar(HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        List<AvatarEntity> avatarEntityList = avatarRepository.findByExpiredLike("F");
        List<AvatarResponse> avatarResponseList = avatarEntityList.stream()
                .map(AvatarResponse::toDto)
                .collect(Collectors.toList());

        return responseUtil.buildSuccessResponse(avatarResponseList);
    }

    // 오브젝트 정보 모두 조회
    @Transactional(readOnly = true)
    public SuccessResponse<List<ObjectResponse>> getAllObject(HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        List<ObjectEntity> objectEntityList = objectRepository.findByExpiredLike("F");
        List<ObjectResponse> objectResponseList = objectEntityList.stream()
                .map(ObjectResponse::toDto)
                .collect(Collectors.toList());

        return responseUtil.buildSuccessResponse(objectResponseList);
    }

    //보상 조회
    @Transactional(readOnly = true)
    public SuccessResponse<List<RewardResponse>> getAllReward(HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        List<RewardEntity> rewardList = rewardRepository.findByExpiredLike("F");
        List<RewardResponse> rewardResponseList = rewardList.stream()
                .map(RewardResponse::toDto)
                .collect(Collectors.toList());

        return responseUtil.buildSuccessResponse(rewardResponseList);
    }

    // 도전과제 모두 조회
    @Transactional(readOnly = true)
    public SuccessResponse<List<ChallengeResponse>> getAllChallenge(HttpServletRequest request){

        String token = Optional.ofNullable(request.getHeader(HttpHeaders.AUTHORIZATION))
                .orElseThrow(() -> new CustomException(ErrorCode.INVALID_AUTH_TOKEN));

        String tokenId = jwtUtil.getUserId(token);

        UserEntity loginUser = userRepository.findByIdAndExpiredLike(Long.valueOf(tokenId), "F")
                .orElseThrow(() -> new CustomException(ErrorCode.USER_NOT_FOUND));

        List<ChallengeEntity> challengeList = challengeRepository.findByExpiredLike("F");
        List<UserRewardEntity> userRewardList = userRewardRepository.findByUser(loginUser);

        List<ChallengeResponse> challengeResponseList = new ArrayList<>();

        for(ChallengeEntity challenge : challengeList){
            String[] achieveNameArr = {challenge.getFirstLevel(), challenge.getSecondLevel(), challenge.getThirdLevel()};
            ChallengeResponse challengeResponse = ChallengeResponse.toDto(challenge, isAchieve(userRewardList, achieveNameArr));
            challengeResponseList.add(challengeResponse);
        }

        return responseUtil.buildSuccessResponse(challengeResponseList);
    }

    @Transactional(readOnly = true)
    public boolean[] isAchieve(List<UserRewardEntity> userRewardList, String[] achieveNameArr){
        boolean singleLevel = false;
        boolean firstLevel = false;
        boolean secondLevel = false;
        boolean thirdLevel = false;

        if("NONE".equals(achieveNameArr[achieveNameArr.length - 1])){
            singleLevel = true;
        }

        for(UserRewardEntity userReward : userRewardList){
            if(!firstLevel && achieveNameArr[0].equals(userReward.getReward().getKeyValue())){
                firstLevel = true;
            }
            if(!secondLevel && achieveNameArr[1].equals(userReward.getReward().getKeyValue())){
                secondLevel = true;
            }
            if(!thirdLevel && achieveNameArr[2].equals(userReward.getReward().getKeyValue())){
                thirdLevel = true;
            }
        }

        return new boolean[]{singleLevel, firstLevel, secondLevel, thirdLevel};
    }
}
