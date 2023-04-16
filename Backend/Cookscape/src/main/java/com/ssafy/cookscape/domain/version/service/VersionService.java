package com.ssafy.cookscape.domain.version.service;

import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import com.ssafy.cookscape.global.error.exception.CustomException;
import com.ssafy.cookscape.global.error.exception.ErrorCode;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.ssafy.cookscape.global.util.ResponseUtil;
import com.ssafy.cookscape.domain.version.db.Entity.VersionEntity;
import com.ssafy.cookscape.domain.version.db.Repository.VersionRepository;

import lombok.RequiredArgsConstructor;

import java.math.BigDecimal;


@Service
@Transactional(readOnly = true) // 기본적으로 트랜잭션 안에서만 데이터 변경하게 설정(성능 향상)
@RequiredArgsConstructor // Lombok을 사용해 @Autowired 없이 의존성 주입. final 객제만 주입됨을 주의
public class VersionService {

    private final VersionRepository versionRepository;
    private final ResponseUtil responseUtil;

    @Transactional
    public SuccessResponse<BigDecimal> getLatestVersion(){

        VersionEntity latestVersion = versionRepository.findTop1ByOrderByIdDesc()
                .orElseThrow(()-> new CustomException(ErrorCode.VERSION_NOT_FOUND));

        return responseUtil.buildSuccessResponse(latestVersion.getVersion());
    }
}

