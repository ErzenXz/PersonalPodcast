import { SimpleForm, Edit, TextInput } from "react-admin";

interface PodcastCreateProps {
   basePath?: string;
}

export const EpisodeEdit = (props: PodcastCreateProps) => {
   return (
      <Edit title="Edit Podcast" {...props}>
         <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="podcastId" />
            <TextInput source="title" />
            <TextInput source="description" />
            <TextInput source="tags" />
            <TextInput source="posterImg" />
            <TextInput source="audioFileUrl" />
            <TextInput source="videoFileUrl" />
            <TextInput source="length" />
            <TextInput source="views" />
            <TextInput source="publisherId" />
         </SimpleForm>
      </Edit>
   );
};

export default EpisodeEdit;
