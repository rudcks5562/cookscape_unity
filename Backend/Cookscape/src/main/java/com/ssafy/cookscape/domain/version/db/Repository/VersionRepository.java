package com.ssafy.cookscape.domain.version.db.Repository;

import com.ssafy.cookscape.domain.version.db.Entity.VersionEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface VersionRepository extends JpaRepository<VersionEntity, Long> {

    Optional<VersionEntity> findTop1ByOrderByIdDesc();
}