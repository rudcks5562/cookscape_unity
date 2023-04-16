package com.ssafy.cookscape.domain.data.model.request;

import com.ssafy.cookscape.domain.data.db.entity.DataEntity;
import com.ssafy.cookscape.domain.data.db.entity.UserAvatarEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@Builder
@ToString
public class GameResultRequest {

    private Long avatarId;
    private int exp;
    private int level;
    private int rankPoint;
    private int money;
    private boolean winFlag;
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

    public DataEntity toDataEntity(DataEntity data){

        return DataEntity.builder()
                .id(data.getId())
                .user(data.getUser())
                .exp(data.getExp() + this.getExp())
                .level(this.getLevel())
                .rankPoint(data.getRankPoint() + this.getRankPoint())
                .money(data.getMoney() + this.getMoney())
                .winCount(this.isWinFlag() ? data.getWinCount() + 1 : data.getWinCount())
                .loseCount(this.isWinFlag() ? data.getLoseCount() : data.getLoseCount() + 1)
                .saveCount(data.getSaveCount() + this.getSaveCount())
                .catchCount(data.getCatchCount() + this.getCatchCount())
                .catchedCount(data.getCatchedCount() + this.getCatchedCount())
                .valveOpenCount(data.getValveOpenCount() + this.getValveOpenCount())
                .valveCloseCount(data.getValveCloseCount() + this.getValveCloseCount())
                .potDestroyCount(data.getPotDestroyCount() + this.getPotDestroyCount())
                .maxNotUseStaminaTime(this.getMaxNotUseStaminaTime() > data.getMaxNotUseStaminaTime() ?
                        this.getMaxNotUseStaminaTime() : data.getMaxNotUseStaminaTime())
                .maxNotMoveTime(this.getMaxNotMoveTime() > data.getMaxNotMoveTime() ?
                        this.getMaxNotMoveTime() : data.getMaxNotMoveTime())
                .maxCatchCount(this.getMaxCatchCount() > data.getMaxCatchCount() ?
                        this.getMaxCatchCount() : data.getMaxCatchCount())
                .maxNotCatchCount(this.getMaxNotCatchCount() > data.getMaxNotCatchCount() ?
                        this.getMaxNotCatchCount() : data.getMaxNotCatchCount())
                .hitedCount(data.getHitedCount() + this.getHitedCount())
                .expired(data.getExpired())
                .build();

    }

    public UserAvatarEntity toUserAvatarEntity(UserAvatarEntity userAvatar){

        return UserAvatarEntity.builder()
                .id(userAvatar.getId())
                .user(userAvatar.getUser())
                .avatar(userAvatar.getAvatar())
                .useCount(userAvatar.getUseCount() + 1)
                .expired(userAvatar.getExpired())
                .build();
    }
}
