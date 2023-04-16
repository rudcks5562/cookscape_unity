package com.ssafy.cookscape.domain.information.model.response;

import com.ssafy.cookscape.domain.information.db.entity.AvatarEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class AvatarResponse {

    private Long avatarId;
    private String name;
    private String desc;
    private float movementSpeed;
    private float rotationSpeed;
    private float jumpForce;
    private float stamina;
    private float staminaDecreasingFactor;
    private float staminaIncreasingFactor;
    private float interactionMinDist;
    private float interactionReadyTime;
    private float footprintSpace;
    private float speedCoFactor;
    private float jumpForceCoFactor;


    public static AvatarResponse toDto(AvatarEntity avatar){

        return AvatarResponse.builder()
                .avatarId(avatar.getId())
                .name(avatar.getName())
                .desc(avatar.getDescription())
                .movementSpeed(avatar.getMovementSpeed())
                .rotationSpeed(avatar.getRotationSpeed())
                .jumpForce(avatar.getJumpForce())
                .stamina(avatar.getStamina())
                .staminaDecreasingFactor(avatar.getStaminaDecreasingFactor())
                .staminaIncreasingFactor(avatar.getStaminaIncreasingFactor())
                .interactionMinDist(avatar.getInteractionMinDist())
                .interactionReadyTime(avatar.getInteractionReadyTime())
                .footprintSpace(avatar.getFootprintSpacer())
                .speedCoFactor(avatar.getSpeedCoFactor())
                .jumpForceCoFactor(avatar.getJumpForceCoFactor())
                .build();

    }
}
