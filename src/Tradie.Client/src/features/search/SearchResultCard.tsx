import { Card, Center, Container, Divider, Grid, Image, Space, Stack, Text } from '@mantine/core';
import { useDispatch, useSelector } from 'react-redux';
import { useAppDispatch } from '../../app/hooks';
import { rounded, substituteValuesInText } from '../../utils/textFormatters';
import { Item } from '../item/item';
import {
  AnalyzerId,
  ItemAffixProperties,
  ItemDetailProperties,
  ItemFlags,
  ItemListingProperties,
  ItemTypeProperties,
} from '../item/itemProperties';
import { selectItemType } from '../itemTypes/itemTypesSlice';
import { Modifier } from '../modifiers/modifier';
import { selectAllModifiers, selectModifier } from '../modifiers/modifiersSlice';
import { notify } from '../notifications/notifications';
import {
  getPropertyDisplayForCategory,
  getPropertyDisplayForRarity,
  ModifierLine,
  ModifierPart,
  PropertyDisplay,
} from './ModifierLine';
import { ModifierSection } from './ModifierSection';
import { ModKind } from './search';
import { SearchResultEntry } from './searchApi';

import styles from './SearchResultCard.module.css';
import { copyWhisperDetails } from './searchSlice';

export interface SearchResultCardProps {
  entry: SearchResultEntry;
  index: number;
}

export function SearchResultCard({ entry, index }: SearchResultCardProps) {
  const item = entry.item;
  const detailProperties = item.findProperty<ItemDetailProperties>(AnalyzerId.ItemDetails);
  const typeProperties = item.findProperty<ItemTypeProperties>(AnalyzerId.ItemType);
  const tradeProperties = item.findProperty<ItemListingProperties>(AnalyzerId.TradeAttributes);
  const affixProperties = item.findProperty<ItemAffixProperties>(AnalyzerId.Modifiers);

  const itemType = useSelector(selectItemType(typeProperties.itemTypeId))!;
  const dispatch = useAppDispatch();

  return (
    <Card
      data-item-id={item.rawId}
      className={`searchResultCard ${index % 2 == 0 ? 'even' : 'odd'}`}
      style={{ height: '100%', margin: 0 }}
    >
      <Grid>
        <Grid.Col span={2}>
          <Center style={{ height: '100%' }}>
            <Image src={itemType.iconUrl} alt={itemType.name} />
          </Center>
        </Grid.Col>
        <Grid.Col span={8}>
          <Center>
            <Stack spacing={5}>
              <Stack spacing={2}>
                {detailProperties.name ? (
                  <>
                    <ModifierLine
                      parts={[
                        { text: detailProperties.name, display: getPropertyDisplayForRarity(detailProperties.rarity) },
                      ]}
                    />
                    <ModifierLine parts={[{ text: itemType.name, display: PropertyDisplay.Name }]} />
                  </>
                ) : (
                  <>
                    <ModifierLine
                      parts={[{ text: itemType.name, display: getPropertyDisplayForCategory(itemType.category) }]}
                    />
                  </>
                )}
              </Stack>
              <div className={styles.separator} />
              <Stack spacing={0}>
                {affixProperties.affixes
                  .filter((c) => c.modifier.location === ModKind.Property)
                  .map((affix, i) => {
                    const modText = useSelector(selectModifier(affix.modifier.modifier))?.text ?? '<unknown modifier>';
                    let parts: ModifierPart[];
                    if (modText.indexOf('#') >= 0) {
                      parts = [
                        {
                          text: substituteValuesInText(modText, affix.value, affix.value),
                          display: PropertyDisplay.Value,
                        },
                      ];
                    } else if (affix.value) {
                      parts = [
                        {
                          text: modText,
                          display: PropertyDisplay.Property,
                        },
                        { text: ': ', display: PropertyDisplay.Property },
                        { text: rounded(affix.value).toString(), display: PropertyDisplay.Value },
                      ];
                    } else {
                      parts = [
                        {
                          text: modText,
                          display: PropertyDisplay.Property,
                        },
                      ];
                    }
                    return <ModifierLine key={i} parts={parts} />;
                  })}
              </Stack>
              <Stack spacing={2}>
                <ModifierSection
                  modifiers={affixProperties.affixes}
                  section={ModKind.Enchant}
                  includeSeparator={false}
                />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Utility} />
                <ModifierSection
                  modifiers={affixProperties.affixes}
                  section={ModKind.Implicit}
                  includeSeparator={false}
                />
                <div className={styles.separator} />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Scourge} />
                <ModifierSection
                  modifiers={affixProperties.affixes}
                  section={ModKind.Fractured}
                  includeSeparator={false}
                />
                <ModifierSection
                  modifiers={affixProperties.affixes}
                  section={ModKind.Explicit}
                  includeSeparator={false}
                />
                <ModifierSection
                  modifiers={affixProperties.affixes}
                  section={ModKind.Veiled}
                  includeSeparator={false}
                />
                <ModifierSection
                  modifiers={affixProperties.affixes}
                  section={ModKind.Crafted}
                  includeSeparator={false}
                />
                <ModifierSection
                  modifiers={affixProperties.affixes}
                  section={ModKind.Pseudo}
                  includeSeparator={false}
                />
                <ModifierSection modifiers={affixProperties.affixes} section={ModKind.Cosmetic} />
                {(detailProperties.flags & ItemFlags.Corrupted) != 0 && (
                  <ModifierLine parts={[{ text: 'Corrupted', display: PropertyDisplay.Corrupted }]} />
                )}
                <div className={styles.separator} />
              </Stack>
              <Stack spacing={0}>
                {tradeProperties.note && (
                  <ModifierLine parts={[{ text: tradeProperties.note, display: PropertyDisplay.Note }]} />
                )}
                <ModifierLine
                  parts={[
                    {
                      text: 'whisper',
                      display: PropertyDisplay.PrimaryAction,
                      onClick: () => {
                        dispatch(copyWhisperDetails(entry));
                      },
                    },
                    { text: ' | ', display: PropertyDisplay.Value },
                    {
                      text: 'history',
                      display: PropertyDisplay.Action,
                      onClick: () => {
                        notify('Not implemented yet');
                      },
                    },
                  ]}
                />
              </Stack>
            </Stack>
          </Center>
        </Grid.Col>
        {/*<Grid.Col span={2}>
          <Center style={{ height: '100%' }}>
            <Stack dir="horizontal" spacing={2}>
              <span className="price-type">{tradeProperties.price.buyoutKind}</span>
              <span className="price">{tradeProperties.price.amount}</span>
            </Stack>
          </Center>
                </Grid.Col>*/}
      </Grid>
    </Card>
  );
}
