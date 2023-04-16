package com.ssafy.cookscape.domain.information.db.repository;

import com.ssafy.cookscape.domain.information.db.entity.RewardEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface RewardRepository extends JpaRepository<RewardEntity, Long> {

    Optional<RewardEntity> findByIdAndExpiredLike(Long id, String expired);

    RewardEntity findByKeyValueLike(String keyValue);

    List<RewardEntity> findByExpiredLike(String expired);
}
