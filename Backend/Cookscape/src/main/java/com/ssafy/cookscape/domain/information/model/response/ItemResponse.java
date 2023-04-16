package com.ssafy.cookscape.domain.information.model.response;

import com.ssafy.cookscape.domain.information.db.entity.ItemEntity;
import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class ItemResponse {

    private Long itemId;
    private String name;
    private String desc;
    private int useCount;
    private float weight;

    public static ItemResponse toDto(ItemEntity item) {

        return ItemResponse.builder()
                .itemId(item.getId())
                .name(item.getName())
                .desc(item.getDescription())
                .useCount(item.getUseCount())
                .weight(item.getWeight())
                .build();

    }
}
