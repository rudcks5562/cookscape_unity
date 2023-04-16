package com.ssafy.cookscape.domain.information.model.response;

import com.ssafy.cookscape.domain.information.db.entity.RewardEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class RewardResponse {

    private Long rewardId;
    private String name;
    private String desc;
    private String type;
    private String keyValue;
    private String grade;

    public static RewardResponse toDto(RewardEntity reward){
        return RewardResponse.builder()
                .rewardId(reward.getId())
                .name(reward.getName())
                .desc(reward.getDescription())
                .type(reward.getType())
                .keyValue(reward.getKeyValue())
                .grade(reward.getGrade())
                .build();
    }
}
