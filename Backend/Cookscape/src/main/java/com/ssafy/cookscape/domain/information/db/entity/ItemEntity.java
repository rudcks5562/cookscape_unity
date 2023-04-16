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
@Table(name = "item")
@Getter
@SuperBuilder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class ItemEntity extends BaseEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", columnDefinition = "INT UNSIGNED")
    private Long id;

    // 아이템 명
    @Column(name = "name", nullable = false, unique = true)
    private String name;

    // 아이템 설명
    @Column(name = "description", length = 1000)
    @ColumnDefault("'아이템 설명이 없습니다.'")
    private String description;

    //  아이템 사용가능 회수
    @Column(name = "use_count", nullable = false)
    @ColumnDefault("1")
    private int useCount;

    // 무게
    @Column(name = "weight", nullable = false)
    @ColumnDefault("1.0")
    private float weight;
}
