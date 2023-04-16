package com.ssafy.cookscape.domain.file.controller;

import com.ssafy.cookscape.domain.file.service.FileService;
import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import io.swagger.models.Response;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.mvc.method.annotation.StreamingResponseBody;

import java.util.concurrent.CompletableFuture;

@Slf4j
@RestController
@RequiredArgsConstructor
@Api(tags = "FileController v1")
@RequestMapping("/v1/file")
public class FileController {

    private final FileService fileService;

    @GetMapping("/app-download/{os}")
    @ApiOperation(value = "앱 다운로드")
    public CompletableFuture<ResponseEntity<StreamingResponseBody>> downladApp(@PathVariable String os){

        return fileService.downloadApp(os);
    }

    @GetMapping("/app/name")
    @ApiOperation(value = "앱 이름 가져오기")
    public ResponseEntity<SuccessResponse> getAppName(){

        SuccessResponse body = fileService.getAppName();

        return ResponseEntity
                .status(HttpStatus.OK)
                .body(body);
    }
}
