import './App.css';
import useDarkMode from 'use-dark-mode';
import { ImSun, ImIcoMoon, ImPencil, ImPlus, ImFloppyDisk } from 'react-icons/im';
import {
  ActionIcon,
  AppShell,
  Avatar,
  Card,
  Center,
  Container,
  Grid,
  Group,
  Header,
  MantineProvider,
  MantineThemeOverride,
  Navbar,
  Text,
} from '@mantine/core';
import { CriteriaGroupCard } from './features/criteriagroups/CriteriaGroupCard';
import { FilterPanel } from './features/filterpanel/FilterPanel';
import { SearchResultList } from './features/search/SearchResultList';
import { useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import { loadCriteria } from './features/criteria/criteriaSlice';
import { loadActiveLeagues, loadDefaultLeague } from './features/leagues/leaguesSlice';
import { loadItemTypes } from './features/itemTypes/itemTypesSlice';
import { NotificationsProvider } from '@mantine/notifications';
import { loadModifiers } from './features/modifiers/modifiersSlice';
import { loadAffixRanges } from './features/affixRanges/affixRangesSlice';

const lightTheme: MantineThemeOverride = {
  colorScheme: 'light',
  fontFamily: "Inter var, -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'sans-serif'",
  primaryColor: 'orange',
};

const darkTheme: MantineThemeOverride = {
  colorScheme: 'dark',
  fontFamily: "Inter var, -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'sans-serif'",
  primaryColor: 'orange',
  defaultRadius: '0px',
};

/*const darkTheme = createTheme({
  type: 'dark',
  theme: {
    colors: {
      primary: '#008aca',
      secondary: '#fea02f',
      sun: '#ff00ff',
    },
  },
});*/

function App() {
  const darkMode = useDarkMode(true);
  const theme = darkMode.value ? darkTheme : lightTheme;
  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(loadCriteria());
    dispatch(loadActiveLeagues());
    dispatch(loadItemTypes());
    dispatch(loadModifiers());
    dispatch(loadAffixRanges());
    dispatch(loadDefaultLeague());
  }, []);
  return (
    <MantineProvider theme={theme} withCSSVariables>
      <NotificationsProvider>
        <AppShell padding="xl" header={
          <Header height="100px" p="xs" className="header">
            <Group align="flex-start">
              <div className="logoContainer">
                <Text
                  component="h1"
                  className="logo"
                  variant="gradient"
                  weight={900}
                  // size="xl"
                  // weight="bold"
                  //gradient={{ from: '#008aca', to: '#006a2a', deg: 45 }}
                  gradient={
                    darkMode.value
                      ? { from: 'orangered', to: 'orange', deg: 45 }
                      : { from: 'orange', to: 'orangered', deg: 45 }
                  }
                >
                  <a href="/">tradie</a>
                </Text>
              </div>
            </Group>
            <Group className="headerRight" position="right" align="flex-end">
              <Center>
                <ActionIcon color={darkMode.value ? 'blue' : 'orange'} onClick={() => darkMode.toggle()}>
                  {darkMode.value ? <ImIcoMoon /> : <ImSun />}
                </ActionIcon>

                <Avatar color="blue">K</Avatar>
              </Center>
            </Group>
          </Header>
        }>
          <Container fluid m={0}>
            <Grid>
              <Grid.Col sm={12} md={5} lg={5} style={{ position: 'sticky', top: 0, left: 0}}>
                <FilterPanel />
              </Grid.Col>
              <Grid.Col sm={12} md={7} lg={7}>
                <Card style={{paddingTop: 0}}>
                  <SearchResultList />
                </Card>
              </Grid.Col>
            </Grid>
          </Container>
        </AppShell>
      </NotificationsProvider>
    </MantineProvider>
  );
}

export default App;
