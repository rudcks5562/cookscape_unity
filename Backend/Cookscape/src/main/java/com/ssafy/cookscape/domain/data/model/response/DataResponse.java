package com.ssafy.cookscape.domain.data.model.response;

import com.ssafy.cookscape.domain.data.db.entity.DataEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class DataResponse {

    private Long dataId;
    private int exp;
    private int level;
    private int rankPoint;
    private int money;
    private int winCount;
    private int loseCount;
    private int saveCount;
    private int catchCount;
    private int catchedCount;
    private int valveOpenCount;
    private int valveCloseCount;
    private int potDestroyCount;
    private float maxNotUseStaminaTime;
    private float maxNotMoveTime;
    private int maxCatchCount;
    private int maxNotCatchCount;
    private int hitedCount;

    public static DataResponse toDto(DataEntity data){

        return DataResponse.builder()
                .dataId(data.getId())
                .exp(data.getExp())
                .level(data.getLevel())
                .rankPoint(data.getRankPoint())
                .money(data.getMoney())
                .winCount(data.getWinCount())
                .loseCount(data.getLoseCount())
                .saveCount(data.getSaveCount())
                .catchCount(data.getCatchCount())
                .catchedCount(data.getCatchedCount())
                .valveOpenCount(data.getValveOpenCount())
                .valveCloseCount(data.getValveCloseCount())
                .potDestroyCount(data.getPotDestroyCount())
                .maxNotUseStaminaTime(data.getMaxNotUseStaminaTime())
                .maxNotMoveTime(data.getMaxNotMoveTime())
                .maxCatchCount(data.getMaxCatchCount())
                .maxNotCatchCount(data.getMaxNotCatchCount())
                .hitedCount(data.getHitedCount())
                .build();
    }
}
