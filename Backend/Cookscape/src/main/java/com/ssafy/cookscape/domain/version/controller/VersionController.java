package com.ssafy.cookscape.domain.version.controller;

import com.ssafy.cookscape.domain.version.service.VersionService;
import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import static org.springframework.http.ResponseEntity.ok;

@Slf4j
@RestController
@RequiredArgsConstructor
@Api(tags = "VersionController v1")
@RequestMapping("/v1/version")
public class VersionController {

    private final VersionService versionService;

    @GetMapping("/latest")
    @ApiOperation(value = "게임 최신 버전 조회")
    public ResponseEntity<SuccessResponse> getVersion(){

        SuccessResponse body = versionService.getLatestVersion();

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }
}
