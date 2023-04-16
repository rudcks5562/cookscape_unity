package com.ssafy.cookscape.domain.file.db.entity;

import com.ssafy.cookscape.global.common.db.entity.BaseEntity;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.experimental.SuperBuilder;
import org.hibernate.annotations.DynamicInsert;

import javax.persistence.*;
import java.math.BigDecimal;

@Entity
@DynamicInsert
@Table(name = "file")
@Getter
@SuperBuilder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class FileEntity extends BaseEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", columnDefinition = "INT UNSIGNED")
    private Long id;

    @Column(name = "file_name", nullable = false)
    private String originFileName;

    @Column(name = "content_type", nullable = false)
    private String contentType;

    @Column(name = "version", precision = 3, scale = 1, unique = true)
    private BigDecimal version;
}
