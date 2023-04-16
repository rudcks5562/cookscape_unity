package com.ssafy.cookscape.domain.data.db.entity;

import com.ssafy.cookscape.global.common.db.entity.BaseEntity;
import com.ssafy.cookscape.domain.information.db.entity.AvatarEntity;
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
@Table(name = "user_avatar")
@Getter
@SuperBuilder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class UserAvatarEntity extends BaseEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", columnDefinition = "INT UNSIGNED")
    private Long id;

    @ManyToOne
    @JoinColumn(name = "user_id", nullable = false)
    private UserEntity user;

    @ManyToOne
    @JoinColumn(name = "avatar_id", nullable = false)
    private AvatarEntity avatar;

    // 사용횟수
    @Column
    @ColumnDefault("0")
    private int useCount;
}
