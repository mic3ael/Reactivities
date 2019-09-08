import React from 'react'
import { Item, Button, Label, Segment } from 'semantic-ui-react';
import { IActivity } from '../../../app/models/activity';

interface IProps {
    activities: IActivity[];
}

const ActivityList: React.FC<IProps> = (props) => {
    const {activities} = props;
    return (
        <Segment clearing>
            <Item.Group divided>
                {activities.map((activity) => {
                    const {title, description, city, venue, date, category} = activity;
                    return (
                        <Item>
                            <Item.Image size='tiny' src='/images/wireframe/image.png' />
                            <Item.Content>
                                <Item.Header as='a'>{title}</Item.Header>
                                <Item.Meta>{date}</Item.Meta>
                                <Item.Description>
                                    <div>{description}</div>
                                    <div>{city}, {venue}</div>
                                </Item.Description>
                                <Item.Extra>
                                    <Button floated="right" content="View" color="blue"/>
                                    <Label basic content={category}/>
                                </Item.Extra>
                            </Item.Content>
                        </Item>
                    );
                })}
            </Item.Group>
        </Segment>
    );
}

export default ActivityList
