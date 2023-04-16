package com.ssafy.cookscape.domain.information.db.repository;

import com.ssafy.cookscape.domain.information.db.entity.AvatarEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface AvatarRepository extends JpaRepository<AvatarEntity, Long> {

    Optional<AvatarEntity> findById(Long id);

    List<AvatarEntity> findByExpiredLike(String expired);
}
