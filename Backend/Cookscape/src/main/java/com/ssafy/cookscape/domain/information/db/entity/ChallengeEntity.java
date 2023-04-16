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
@Table(name = "challenge")
@Getter
@SuperBuilder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class ChallengeEntity extends BaseEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", columnDefinition = "INT UNSIGNED")
    private Long id;

    @Column(name = "name", nullable = false)
    private String name;

    @Column(name = "description", nullable = false)
    private String description;

    @Column(name = "first_level", nullable = false)
    @ColumnDefault("'NONE'")
    private String firstLevel;

    @Column(name = "second_level", nullable = false)
    @ColumnDefault("'NONE'")
    private String secondLevel;

    @Column(name = "third_level", nullable = false)
    @ColumnDefault("'NONE'")
    private String thirdLevel;

    @Column(name = "key_value", nullable = false, unique = true)
    private String keyValue;
}
