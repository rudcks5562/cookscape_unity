package com.ssafy.cookscape.global.auth.service;

import com.ssafy.cookscape.domain.user.db.entity.UserEntity;
import com.ssafy.cookscape.domain.user.db.repository.UserRepository;
import com.ssafy.cookscape.global.auth.model.PrincipalDetails;
import lombok.RequiredArgsConstructor;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class CustomUserDetailsService implements UserDetailsService {

    private final UserRepository userRepository;

    @Override
    public UserDetails loadUserByUsername(String username) throws UsernameNotFoundException {
        // 들어온 username은 email이므로 loadUserByEmail로 바꿔줌
        // email이 아닌 다른 key를 통해 값을 넘겨 줄거면 메소드를 구현해서 넘겨 줄 것
        return loadUserByLoginId(username);
    }

    /**
     * 유저의 정보를 불러와서 UserDetails로 리턴
     *
     * @param
     * @return UserDetails
     * @throws UsernameNotFoundException
     */
    public UserDetails loadUserByLoginId(String loginId) throws UsernameNotFoundException {
        UserEntity loadUser = userRepository.findByLoginIdAndExpiredLike(loginId, "F")
                .orElseThrow(() -> new UsernameNotFoundException("사용자를 찾을 수 없습니다. ID =" + loginId));

        PrincipalDetails principalDetails = new PrincipalDetails(loadUser);

        return principalDetails;
    }
}
