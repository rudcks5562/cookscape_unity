package com.ssafy.cookscape.domain.file.db.repository;

import com.ssafy.cookscape.domain.file.db.entity.FileEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.math.BigDecimal;
import java.util.Optional;

public interface FileRepository extends JpaRepository<FileEntity, Long> {

    Optional<FileEntity> findByVersion(BigDecimal version);
}
