package com.ssafy.cookscape.domain.data.db.repository;

import com.ssafy.cookscape.domain.data.db.entity.UserRewardEntity;
import com.ssafy.cookscape.domain.information.db.entity.RewardEntity;
import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface UserRewardRepository extends JpaRepository<UserRewardEntity, Long> {

    List<UserRewardEntity> findByUser(UserEntity user);

    UserRewardEntity findByUserAndReward(UserEntity user, RewardEntity reward);
}
