package com.ssafy.cookscape.domain.information.model.response;

import com.ssafy.cookscape.domain.information.db.entity.ObjectEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class ObjectResponse {

    private Long objectId;
    private String name;
    private String desc;
    private float gauge;
    private float chargingSpeed;
    private float dechargingSpeed;
    private String interactableType;
    private float chargingSpeedCoFactor;
    private float deChargingSpeedCoFactor;

    public static ObjectResponse toDto(ObjectEntity object){
        return ObjectResponse.builder()
                .objectId(object.getId())
                .name(object.getName())
                .desc(object.getDescription())
                .gauge(object.getGauge())
                .chargingSpeed(object.getChargingSpeed())
                .dechargingSpeed(object.getDechargingSpeed())
                .interactableType(object.getInteractableType())
                .chargingSpeedCoFactor(object.getChargingSpeedCoFactor())
                .deChargingSpeedCoFactor(object.getDeChargingSpeedCoFactor())
                .build();
    }
}
