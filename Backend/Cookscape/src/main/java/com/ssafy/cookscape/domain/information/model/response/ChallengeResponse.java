package com.ssafy.cookscape.domain.information.model.response;

import com.ssafy.cookscape.domain.information.db.entity.ChallengeEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class ChallengeResponse {

    private Long challengeId;
    private String name;
    private String desc;
    private boolean singleAchievementFlag;
    private String firstLevel;
    private boolean firstAchievementFlag;
    private String secondLevel;
    private boolean secondAchievementFlag;
    private String thirdLevel;
    private boolean thirdAchievementFlag;
    private String keyValue;

    public static ChallengeResponse toDto(ChallengeEntity challenge, boolean[] flag){
        return ChallengeResponse.builder()
                .challengeId(challenge.getId())
                .name(challenge.getName())
                .desc(challenge.getDescription())
                .firstLevel(challenge.getFirstLevel())
                .secondLevel(challenge.getSecondLevel())
                .thirdLevel(challenge.getThirdLevel())
                .singleAchievementFlag((flag[0]))
                .firstAchievementFlag(flag[1])
                .secondAchievementFlag(flag[2])
                .thirdAchievementFlag(flag[3])
                .keyValue(challenge.getKeyValue())
                .build();

    }
}
