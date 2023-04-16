package com.ssafy.cookscape.domain.user.db.repository;

import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface UserRepository extends JpaRepository<UserEntity, Long> {

    UserEntity findByLoginId(String loginId);

    UserEntity findByNickname(String nickname);

    Optional<UserEntity> findByLoginIdAndExpiredLike(String loginId, String expired);

    Optional<UserEntity> findByIdAndExpiredLike(Long id, String expired);
}
