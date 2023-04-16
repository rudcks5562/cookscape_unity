package com.ssafy.cookscape.global.auth.model;

import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Map;

@Getter
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@ToString
public class PrincipalDetails implements UserDetails, Serializable {

    private static final long serialVersionUID = 1L;

    // Spring Security는 “ROLE_” 형태로 인식하기에 perfix를 붙여줄 것
    private static final String ROLE_PREFIX = "ROLE_USER";

    private UserEntity user;
    private Collection<SimpleGrantedAuthority> authorities;
    //	private Map<String, Object> attributes;

    // UserDetails - Form 로그인 시 사용
    public PrincipalDetails(UserEntity user) {
        this.user = user;
    }

    // common fields
    @Override
    public Collection<? extends GrantedAuthority> getAuthorities() {
        // 권한이 단일 권한이라 간단히 처리
        // 리스트 형식으로 주어지는 다중 권한이면 parser를 만들어서 담아야 함
        SimpleGrantedAuthority authority = new SimpleGrantedAuthority(ROLE_PREFIX);

        authorities = new ArrayList<>();
        authorities.add(authority);
        return authorities;
    }

    public String getLoginId() {
        return user.getLoginId();
    }

    // UserDetails Fields
    @Override
    public String getPassword() {
        return user.getPassword();
    }

    @Override
    public String getUsername() {
        return user.getNickname();
    }

    /**
     * 계정 만료 여부 <br>
     *
     * true : 만료 안됨 / false : 만료
     * @return
     */
    @Override
    public boolean isAccountNonExpired() {
        if ("F".equals(this.user.getExpired())) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * 계정 잠김 여부 <br>
     *
     * true : 잠기지 않음 / false : 잠김
     * @return
     */
    @Override
    public boolean isAccountNonLocked() {
        return true;
    }

    /**
     * 비밀번호 만료 여부 <br>
     *
     * true : 만료 안됨 / false : 만료
     * @return
     */
    @Override
    public boolean isCredentialsNonExpired() {
        return true;
    }

    /**
     * 사용자 활성화 여부 <br>
     *
     * true : 활성화 / false : 비활성화
     * @return
     */
    @Override
    public boolean isEnabled() {
        return true;
    }

}
