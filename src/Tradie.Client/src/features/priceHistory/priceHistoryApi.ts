import { get } from "../../api/apiClient";
import { HistoricalPrice } from "./historicalPrice";

export const getPriceHistoryForItem = (itemId: string): HistoricalPrice[] => {
  await get($'priceHistory/{itemId}')
}
