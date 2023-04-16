package com.ssafy.cookscape.domain.file.service;

import com.ssafy.cookscape.domain.file.db.entity.FileEntity;
import com.ssafy.cookscape.domain.file.db.repository.FileRepository;
import com.ssafy.cookscape.domain.version.db.Entity.VersionEntity;
import com.ssafy.cookscape.domain.version.db.Repository.VersionRepository;
import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import com.ssafy.cookscape.global.error.exception.CustomException;
import com.ssafy.cookscape.global.util.ResponseUtil;
import com.ssafy.cookscape.global.error.exception.ErrorCode;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.io.FileSystemResource;
import org.springframework.core.io.Resource;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.servlet.mvc.method.annotation.StreamingResponseBody;

import java.io.IOException;
import java.io.InputStream;
import java.net.URLEncoder;
import java.util.concurrent.CompletableFuture;

@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor // Lombok을 사용해 @Autowired 없이 의존성 주입. final 객제만 주입됨을 주의
public class FileService {

    private final ResponseUtil responseUtil;
    private final FileRepository fileRepository;
    private final VersionRepository versionRepository;

    @Value("${file.app.windowPath}")
    private String appWindowPath;

    @Async
    public CompletableFuture<ResponseEntity<StreamingResponseBody>> downloadApp(String os) {

        VersionEntity latestVersion = versionRepository.findTop1ByOrderByIdDesc()
                .orElseThrow(() -> new CustomException(ErrorCode.VERSION_NOT_FOUND));

        FileEntity appFile = fileRepository.findByVersion(latestVersion.getVersion())
                .orElseThrow(() -> new CustomException(ErrorCode.FILE_NOT_FOUND));

        String filePath = null;

        if ("window".equals(os)) {
            filePath = appWindowPath + appFile.getOriginFileName();
        } else {
            throw new CustomException(ErrorCode.INVALID_PARAMS);
        }

        Resource fileResource = new FileSystemResource(filePath);
        if (!fileResource.exists()) {
            throw new CustomException(ErrorCode.INVALID_FILE_PATH);
        }

        String encodedFileName = null;
        StreamingResponseBody responseBody = null;
        try {
            encodedFileName = URLEncoder.encode(appFile.getOriginFileName(), "UTF-8");
            InputStream inputStream = fileResource.getInputStream();
            responseBody = outputStream -> {
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = inputStream.read(buffer)) != -1) {
                    outputStream.write(buffer, 0, bytesRead);
                }
            };

        } catch (IOException e) {
            throw new CustomException(ErrorCode.INTERNAL_SERVER_ERROR);
        }

        return CompletableFuture.completedFuture(
                ResponseEntity.ok()
                        .header(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"" + encodedFileName + "\";")
                        .header(HttpHeaders.CONTENT_TYPE, MediaType.APPLICATION_OCTET_STREAM_VALUE)
                        .body(responseBody));
    }

    public SuccessResponse<String> getAppName(){

        VersionEntity latestVersion = versionRepository.findTop1ByOrderByIdDesc()
                .orElseThrow(() -> new CustomException(ErrorCode.VERSION_NOT_FOUND));

        FileEntity appFile = fileRepository.findByVersion(latestVersion.getVersion())
                .orElseThrow(() -> new CustomException(ErrorCode.FILE_NOT_FOUND));

        return responseUtil.buildSuccessResponse(appFile.getOriginFileName());
    }
}
