import { SimpleForm, Create, TextInput, DateInput } from "react-admin";

interface PodcastCreateProps {
   basePath?: string;
}

export const CommentCreate = (props: PodcastCreateProps) => {
   return (
      <Create title="Create a Comment" {...props}>
         <SimpleForm>
            <TextInput source="userId" />
            <TextInput source="episodeId" />
            <DateInput source="date" />
            <TextInput source="message" />
         </SimpleForm>
      </Create>
   );
};

export default CommentCreate;
