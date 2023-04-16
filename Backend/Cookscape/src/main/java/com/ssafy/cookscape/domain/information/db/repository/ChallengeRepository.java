package com.ssafy.cookscape.domain.information.db.repository;

import com.ssafy.cookscape.domain.information.db.entity.ChallengeEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ChallengeRepository extends JpaRepository<ChallengeEntity, Long> {

    List<ChallengeEntity> findByExpiredLike(String expired);
}
