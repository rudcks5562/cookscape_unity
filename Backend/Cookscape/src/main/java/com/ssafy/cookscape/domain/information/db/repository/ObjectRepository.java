package com.ssafy.cookscape.domain.information.db.repository;

import com.ssafy.cookscape.domain.information.db.entity.ObjectEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ObjectRepository extends JpaRepository<ObjectEntity, Long> {

    List<ObjectEntity> findByExpiredLike(String expired);
}
