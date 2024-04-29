import { SimpleForm, Create, TextInput, DateInput } from "react-admin";

interface PodcastCreateProps {
   basePath?: string;
}

export const PodcastCreate = (props: PodcastCreateProps) => {
   return (
      <Create title="Create a Podcast" {...props}>
         <SimpleForm>
            <TextInput source="title" />
            <TextInput source="description" />
            <TextInput source="categoryId" />
            <TextInput source="tags" />
            <TextInput source="posterImg" />
            <TextInput source="audioFileUrl" />
            <TextInput source="videoFileUrl" />
            <TextInput source="publisherId" />
            <DateInput source="createdDate" />
         </SimpleForm>
      </Create>
   );
};

export default PodcastCreate;
