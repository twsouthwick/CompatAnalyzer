import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface HistoryState {
    issues: IssueResult[];
    loading: boolean;
}

export class History extends React.Component<RouteComponentProps<{}>, HistoryState> {
    constructor() {
        super();
        this.state = { issues: [], loading: true };

        fetch('api/analyzer')
            .then(response => response.json() as Promise<IssueResult[]>)
            .then(data => {
                this.setState({ issues: data, loading: false });
            });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : History.renderIssues(this.state.issues);

        return <div>
            <h1>Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>;
    }

    private static renderIssues(issues: IssueResult[]) {
        //let contents = issues.map(issue => History.renderIssueResult(issue));

        return <div>
            <h1>Hello!</h1>
        </div>;
    }

    private static renderIssueResult(result: IssueResult) {
        return <div>
            <h2>{result.id} ({result.state})</h2>
            {History.renderIssue(result.issues)}
        </div>;
    }

    private static renderIssue(issues: Issue[]): JSX.Element {
        let table = <table className='table'>
            <thead>
                <tr>
                    <th>Id</th>
                    <th>Message</th>
                </tr>
            </thead>

            <tbody>
                {issues.map(issue =>
                    <tr key={issue.id}>
                        <td>{issue.message}</td>
                    </tr>
                )}
            </tbody>
        </table>;

        return <div>
            {table}
            {issues.filter(t => t.nested.length > 0)
                .map(issue => History.renderIssue(issue.nested))
            }
        </div >
    }
}

interface Issue {
    id: string,
    message: string,
    nested: Issue[]
}

interface IssueResult {
    id: string,
    issues: Issue[];
    state: number;
}
