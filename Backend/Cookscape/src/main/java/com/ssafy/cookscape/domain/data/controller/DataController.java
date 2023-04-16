package com.ssafy.cookscape.domain.data.controller;

import com.ssafy.cookscape.domain.data.model.request.GameResultRequest;
import com.ssafy.cookscape.domain.data.service.DataService;
import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import io.swagger.models.Response;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpServletRequest;

@Slf4j
@RestController
@RequiredArgsConstructor
@Api(tags = "DataController v1")
@RequestMapping("/v1/data")
public class DataController {

    private final DataService dataService;

    @GetMapping("/usage-avatar")
    @ApiOperation(value = "아바타 사용횟수 조회")
    public ResponseEntity<SuccessResponse> getUsageAvatarData(HttpServletRequest request){

        SuccessResponse body = dataService.getUsageAvatarData(request);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }

    @GetMapping("/user-data")
    @ApiOperation(value = "유저 데이터 조회")
    public ResponseEntity<SuccessResponse> getUserData(HttpServletRequest request){

        SuccessResponse body = dataService.getUserData(request);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }

    @PutMapping("/result")
    @ApiOperation(value = "게임 결과 데이터 업데이트")
    public ResponseEntity<SuccessResponse> updateResultData(@RequestBody GameResultRequest resultDto, HttpServletRequest request){

        SuccessResponse body = dataService.updateResultData(resultDto, request);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(body);
    }

    // 리워드 등록
    @PostMapping("/rewards/{rewardId}")
    @ApiOperation(value = "보상 수령 등록")
    public ResponseEntity<SuccessResponse> registReward(@PathVariable Long rewardId, HttpServletRequest request){

        SuccessResponse body = dataService.receiveReward(rewardId, request);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(body);
    }

    // 보유한 모든 보상 조회
    @GetMapping("/rewards")
    @ApiOperation(value = "보유/미보유 보상 조회")
    public ResponseEntity<SuccessResponse> getRewardBy(
            @RequestParam(value = "isPossession", required = true) Boolean isPossession,
            HttpServletRequest request){

        SuccessResponse body = dataService.getRewardByConditon(isPossession, request);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }


}
