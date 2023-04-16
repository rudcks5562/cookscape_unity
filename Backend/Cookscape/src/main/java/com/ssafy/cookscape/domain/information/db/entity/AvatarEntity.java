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
@Table(name = "avatar")
@Getter
@SuperBuilder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class AvatarEntity extends BaseEntity {

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
    @ColumnDefault("'아바타 설명이 없습니다.'")
    private String description;

    // 캐릭터 이동속도
    @Column(name = "movement_speed", nullable = false)
    @ColumnDefault("11.0")
    private float movementSpeed;

    // 캐릭터 회전속도
    @Column(name = "rotation_speed", nullable = false)
    @ColumnDefault("200.0")
    private float rotationSpeed;

    // 아바타 점프력
    @Column(name = "jump_force", nullable = false)
    @ColumnDefault("20.0")
    private float jumpForce;

    // 최대 스태미너
    @Column(name = "stamina", nullable = false)
    @ColumnDefault("100.0")
    private float stamina;

    // 스태미너 감소 속도
    @Column(name = "stamina_decreasing_factor", nullable = false)
    @ColumnDefault("10.0")
    private float staminaDecreasingFactor;

    // 스태미너 증가 속도
    @Column(name = "stamina_increasing_factor", nullable = false)
    @ColumnDefault("1.0")
    private float staminaIncreasingFactor;

    // 상호작용 최소거리
    @Column(name = "interaction_min_dist", nullable = false)
    @ColumnDefault("5.0")
    private float interactionMinDist;

    // 상호작용 준비시간
    @Column(name = "interaction_ready_time", nullable = false)
    @ColumnDefault("0.2")
    private float interactionReadyTime;

    // 발자국 간격
    @Column(name = "footprint_space", nullable = false)
    @ColumnDefault("1.0")
    private float footprintSpacer;

    // 이동속도 보정값
    @Column(name = "speed_co_factor", nullable = false)
    @ColumnDefault("1.0")
    private float speedCoFactor;

    // 점프력 보정값
    @Column(name = "jump_force_co_factor", nullable = false)
    @ColumnDefault("1.0")
    private float jumpForceCoFactor;
}
