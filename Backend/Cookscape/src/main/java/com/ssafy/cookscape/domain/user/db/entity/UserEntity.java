package com.ssafy.cookscape.domain.user.db.entity;

import com.ssafy.cookscape.domain.information.db.entity.AvatarEntity;
import com.ssafy.cookscape.global.common.db.entity.BaseEntity;
import com.ssafy.cookscape.domain.data.db.entity.DataEntity;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.experimental.SuperBuilder;
import org.hibernate.annotations.ColumnDefault;
import org.hibernate.annotations.DynamicInsert;

import javax.persistence.*;

@Entity
@DynamicInsert
@Table(name = "user")
@Getter
@SuperBuilder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class UserEntity extends BaseEntity {

    private static final long serialVersionUID = 1L;

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", columnDefinition = "INT UNSIGNED")
    private Long id;

    // 유저에게 연결된 데이터
    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "data_id", nullable = false)
    private DataEntity data;

    // 로그인에 사용되는 아이디
    @Column(name = "login_id", nullable = false, unique = true)
    private String loginId;

    // 비밀번호
    @Column(name = "password", nullable = false)
    private String password;

    // 닉네임
    @Column(name = "nickname", nullable = false, unique = true)
    private String nickname;

    // 유저가 사용중인 메타버스 아바타
    @Column(name = "avatar_name", nullable = false)
    private String avatarName;

    // 유저가 사용중인 칭호
    @Column(name = "title", nullable = false)
    @ColumnDefault("'무기농'")
    private String title;

    // 유저가 사용중인 모자
    @Column(name = "hat", nullable = false)
    @ColumnDefault("'NONE'")
    private String hat;
}
