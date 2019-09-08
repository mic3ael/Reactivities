import React, { useState, useEffect, Fragment } from 'react';
import { Container } from 'semantic-ui-react';
import { IActivity } from '../models/activity';
import axios from "axios";
import NavBar from '../../features/nav/NavBar';
import './styles.css';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';

const App = () => {
  const [activities, setActivities] = useState<IActivity[]>([]);
  useEffect(() => {
  
    async function fetchActivities(){
      const res = await axios.get<IActivity[]>('http://localhost:5000/api/activities');
      setActivities(res.data);
    }

    fetchActivities();
  }, []);

    return (
    <Fragment>
      <NavBar/>
      <Container style={{marginTop: '7em'}}>
        <ActivityDashboard activities={activities}/>
      </Container>
    </Fragment>
    );
  };

export default App;