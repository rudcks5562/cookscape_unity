package com.ssafy.cookscape.global.auth;

import com.ssafy.cookscape.global.auth.model.PrincipalDetails;
import com.ssafy.cookscape.global.auth.service.CustomUserDetailsService;
import com.ssafy.cookscape.global.util.CookieUtil;
import com.ssafy.cookscape.global.util.JwtUtil;
import io.jsonwebtoken.ExpiredJwtException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpHeaders;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import javax.servlet.FilterChain;
import javax.servlet.ServletException;
import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import java.io.IOException;

@Slf4j
@Component
@RequiredArgsConstructor
public class JwtRequestFilter extends OncePerRequestFilter {

    private final JwtUtil jwtUtil;
    private final CookieUtil cookieUtil;

    private final CustomUserDetailsService customUserDetailsService;

    @Override
    protected void doFilterInternal(HttpServletRequest request, HttpServletResponse response,
                                    FilterChain filterChain) throws ServletException, IOException {

        String accessJwt = null, refreshJwt = null, validateJwt = null;
        String accessLoginId = null, refreshLoginId = null, validateLoginId = null;

        // // 쿠키에서 accessToken을 사용할 때 가지고 오는 메소드. 사용하지 않고 http header AUTHORIZATION에서 가져올 것
        // Cookie accessToken = cookieUtil.getCookie(httpServletRequest, jwtUtil.ACCESS_TOKEN);

        try {
            accessJwt = jwtUtil.getAuthToken(request);
            accessLoginId = jwtUtil.getUserLoginId(accessJwt);

            if (accessLoginId != null) {
                PrincipalDetails userDetails = (PrincipalDetails) customUserDetailsService
                        .loadUserByUsername(accessLoginId);

                if (jwtUtil.validateToken(accessJwt, userDetails)) {
                    UsernamePasswordAuthenticationToken authenticationToken = new UsernamePasswordAuthenticationToken(
                            userDetails, null, userDetails.getAuthorities());

                    authenticationToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

                    SecurityContextHolder.getContext().setAuthentication(authenticationToken);
                }
            }
        } catch (ExpiredJwtException e) {

            Cookie refreshCookie = cookieUtil.getCookie(request, jwtUtil.REFRESH_TOKEN);

            if (refreshCookie != null) {
                refreshJwt = refreshCookie.getValue();
            }

        } catch (Exception e) {

        }

        // 원래 이런방식으로 reissue 재발급을 받지 않음. 프론트의 axios 단에서 인터셉트해서 분기 처리해주어야 함
        // refresh token을 reissue 해주는 메소드
        try {
            if (refreshJwt != null) {

                refreshLoginId = jwtUtil.getUserLoginId(refreshJwt);

                if (validateJwt != null) {
                    validateLoginId = jwtUtil.getUserLoginId(validateJwt);
                }

                if (refreshLoginId.equals(validateLoginId)) {
                    PrincipalDetails userDetails = (PrincipalDetails) customUserDetailsService
                            .loadUserByUsername(refreshLoginId);

                    UsernamePasswordAuthenticationToken authenticationToken = new UsernamePasswordAuthenticationToken(
                            userDetails, null, userDetails.getAuthorities());

                    authenticationToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

                    SecurityContextHolder.getContext().setAuthentication(authenticationToken);

                    String newToken = jwtUtil.createAccessToken(userDetails.getUser());

                    //	Cookie newAccessToken = cookieUtil.createCookie(jwtUtil.ACCESS_TOKEN, newToken);
                    //	httpServletResponse.addCookie(newAccessToken);
                    response.setHeader(HttpHeaders.AUTHORIZATION, newToken);
                }
            }

        } catch (ExpiredJwtException e) {

        } catch (Exception e) {

        }

        filterChain.doFilter(request, response);
    }
}
