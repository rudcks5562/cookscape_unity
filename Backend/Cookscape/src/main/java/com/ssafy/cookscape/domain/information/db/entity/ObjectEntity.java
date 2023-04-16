package com.ssafy.cookscape.domain.information.db.entity;

import com.ssafy.cookscape.global.common.db.entity.BaseEntity;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.experimental.SuperBuilder;
import org.hibernate.annotations.ColumnDefault;
import org.hibernate.annotations.DynamicInsert;

import javax.persistence.*;

@Entity
@DynamicInsert
@Table(name = "object")
@Getter
@SuperBuilder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class ObjectEntity extends BaseEntity {

    private static final long serialVersionUID = 1L;

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", columnDefinition = "INT UNSIGNED")
    private Long id;

    // 아바타 이름
    @Column(name = "name", nullable = false, unique = true)
    private String name;

    // 아바타 설명
    @Column(name = "description", length = 1000)
    @ColumnDefault("'오브젝트 설명이 없습니다.'")
    private String description;

    // 기본 게이지
    @Column(name = "gauge", nullable = false)
    @ColumnDefault("0")
    private float gauge;

    // 차징 속도
    @Column(name = "charging_speed", nullable = false)
    @ColumnDefault("1.0")
    private float chargingSpeed;

    // 디차징 속도
    @Column(name = "decharging_speed", nullable = false)
    @ColumnDefault("2.0")
    private float dechargingSpeed;

    // 상호작용 타입
    @Column(name = "interactable_type", nullable = false)
    @ColumnDefault("'ALL'")
    private String interactableType;

    // 차징 보정속도
    @Column(name = "charging_speed_co_factor", nullable = false)
    @ColumnDefault("1.0")
    private float chargingSpeedCoFactor;

    // 디차징 보정속도
    @Column(name = "decharging_speed_co_factor", nullable = false)
    @ColumnDefault("2.0")
    private float deChargingSpeedCoFactor;

}
