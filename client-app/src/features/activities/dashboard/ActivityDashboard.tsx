import React from 'react'
import { Grid } from 'semantic-ui-react';
import { IActivity } from '../../../app/models/activity';
import ActivityList from "./ActivityList";
import ActivityDetails from '../details/ActivityDetails';

interface IProps {
    activities: IActivity[];
}

const ActivityDashboard: React.FC<IProps> = (props) => {
    const {activities} = props;
    return (
        <Grid>
            <Grid.Column width={10}>
                <ActivityList activities={activities}/>
            </Grid.Column>
            <Grid.Column width={6}>
                    <ActivityDetails/>
                </Grid.Column>
        </Grid>
    );
}

export default ActivityDashboard
