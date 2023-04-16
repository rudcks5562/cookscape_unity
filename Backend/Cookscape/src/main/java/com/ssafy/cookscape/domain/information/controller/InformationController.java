package com.ssafy.cookscape.domain.information.controller;

import com.ssafy.cookscape.domain.information.model.response.ObjectResponse;
import com.ssafy.cookscape.domain.information.service.InformationService;
import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import javax.servlet.http.HttpServletRequest;
import java.util.List;

@Slf4j
@RestController
@RequiredArgsConstructor
@Api(tags = "InformationController v1")
@RequestMapping("/v1/information")
public class InformationController {

    private final InformationService informationService;

    // 아이템 모두 조회
    @GetMapping("/item")
    @ApiOperation(value = "게임 아이템 정보 모두 조회")
    public ResponseEntity<SuccessResponse> getAllItem(HttpServletRequest request){

        SuccessResponse body = informationService.getAllItem(request);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }


    // 아바타 모두 조회
    @GetMapping("/avatar")
    @ApiOperation(value = "게임 아바타 정보 모두 조회")
    public ResponseEntity<SuccessResponse> getAllAvatar(HttpServletRequest request){

        SuccessResponse body = informationService.getAllAvatar(request);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }

    // 오브젝트 모두 조회
    @GetMapping("/object")
    @ApiOperation(value = "오브젝트 정보 모두 조회")
    public ResponseEntity<SuccessResponse> getAllObject(HttpServletRequest request){

        SuccessResponse body = informationService.getAllObject(request);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }

    // 보상 모두 조회
    @GetMapping("/reward")
    @ApiOperation(value = "보상 정보 모두 조회")
    public ResponseEntity<SuccessResponse> getAllReward(HttpServletRequest request){

        SuccessResponse body = informationService.getAllReward(request);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }

    // 도전과제 모두 조회
    @GetMapping("/challenge")
    @ApiOperation(value = "도전과제 정보 모두 조회")
    public ResponseEntity<SuccessResponse> getAllChallenge(HttpServletRequest request){

        SuccessResponse body = informationService.getAllChallenge(request);

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }
}
