package com.ssafy.cookscape.domain.data.db.entity;

import com.ssafy.cookscape.global.common.db.entity.BaseEntity;
import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.experimental.SuperBuilder;
import org.hibernate.annotations.ColumnDefault;
import org.hibernate.annotations.DynamicInsert;

import javax.persistence.*;

@Entity
@DynamicInsert
@Table(name = "data")
@Getter
@SuperBuilder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class DataEntity extends BaseEntity {

    private static final long serialVersionUID = 1L;

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", columnDefinition = "INT UNSIGNED")
    private Long id;

    @OneToOne(mappedBy = "data", fetch = FetchType.LAZY, cascade = CascadeType.ALL)
    private UserEntity user;

    //경험치
    @Column(name = "exp")
    @ColumnDefault("0")
    private int exp;

    //레벨
    @Column(name = "level")
    @ColumnDefault("1")
    private int level;

    //랭크점수
    @Column(name = "rank_point")
    @ColumnDefault("1000")
    private int rankPoint;

    // 골드
    @Column(name = "money")
    @ColumnDefault("0")
    private int money;

    //승리 횟수
    @Column(name = "win_count")
    @ColumnDefault("0")
    private int winCount;

    //패배 횟수
    @Column(name = "lose_count")
    @ColumnDefault("0")
    private int loseCount;

    // 구한 횟수
    @Column(name = "save_count")
    @ColumnDefault("0")
    private int saveCount;

    // 잡은 횟수
    @Column(name = "catch_count")
    @ColumnDefault("0")
    private int catchCount;

    // 잡힌 횟수
    @Column(name = "catched_count")
    @ColumnDefault("0")
    private int catchedCount;

    // 밸브 연 횟수
    @Column(name = "valve_open_count")
    @ColumnDefault("0")
    private int valveOpenCount;

    // 밸브 닫은 횟수
    @Column(name = "valve_close_count")
    @ColumnDefault("0")
    private int valveCloseCount;

    // 냄비 넘어뜨린 횟수
    @Column(name = "pot_destroy_count")
    @ColumnDefault("0")
    private int potDestroyCount;

    // 한게임에 스태미너를 소모하지 않은 최대 시간
    @Column(name = "max_not_use_stamina_time")
    @ColumnDefault("0")
    private float maxNotUseStaminaTime;

    // 맞은 횟수
    @Column(name = "hited_count")
    @ColumnDefault("0")
    private int hitedCount;

    // 한게임에 움직이지 않은 최대 시간
    @Column(name ="max_not_move_time")
    @ColumnDefault("0")
    private float maxNotMoveTime;

    // 한 게임에서 최대로 잡은 식재료 수
    @Column(name ="max_catch_count")
    @ColumnDefault("0")
    private int maxCatchCount;

    // 한 게임에서 기절만 시키고 잡지않은 개수
    @Column(name ="max_not_catch_count")
    @ColumnDefault("0")
    private int maxNotCatchCount;
}
