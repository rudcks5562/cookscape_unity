package com.ssafy.cookscape.domain.information.db.repository;

import com.ssafy.cookscape.domain.information.db.entity.ItemEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ItemRepository extends JpaRepository<ItemEntity, Long> {

    List<ItemEntity> findByExpiredLike(String expired);
}
